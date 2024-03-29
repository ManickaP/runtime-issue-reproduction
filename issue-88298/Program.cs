﻿using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Net.Quic;
using System.Net;
using System.Text;
using System.Net.Security;

var cert = CreateSelfSignedCertificate();

await using var listener = await QuicListener.ListenAsync(new QuicListenerOptions
{
    ApplicationProtocols = new List<SslApplicationProtocol>
    {
        new SslApplicationProtocol("test")
    },
    ListenEndPoint = IPEndPoint.Parse("127.0.0.1:19999"),
    ConnectionOptionsCallback = (con, hello, token) =>
    {
        Console.WriteLine("Called!");
        return ValueTask.FromResult(new QuicServerConnectionOptions
        {
            DefaultStreamErrorCode = 123456,
            DefaultCloseErrorCode = 654321,
            ServerAuthenticationOptions = new SslServerAuthenticationOptions
            {
                ApplicationProtocols = new List<SslApplicationProtocol>
                {
                    new SslApplicationProtocol("test")
                },
                ServerCertificate = cert,
                RemoteCertificateValidationCallback = (sender, chain, certificate, errors) => true
            },
        });
    },
});


_ = Task.Run(async () =>
{
    await using var con = await listener.AcceptConnectionAsync();
    await using var stream = await con.AcceptInboundStreamAsync();
    var reader = new StreamReader(stream);
    while (true)
    {
        var data = await reader.ReadLineAsync();
        Console.WriteLine(data);
        await stream.WriteAsync(Encoding.UTF8.GetBytes("World\n"));
    }
});

try
{

    var options = new QuicClientConnectionOptions
    {
        RemoteEndPoint = IPEndPoint.Parse("127.0.0.1:19999"),
        DefaultCloseErrorCode = 789,
        DefaultStreamErrorCode = 987,
        ClientAuthenticationOptions = new SslClientAuthenticationOptions
        {
            ApplicationProtocols = new List<SslApplicationProtocol>
            {
                new SslApplicationProtocol("test")
            },
            ClientCertificates = new X509CertificateCollection { cert },
            TargetHost = "localhost",
            RemoteCertificateValidationCallback = (sender, chain, certificate, errors) => true
        }
    };
    await using var value = await QuicConnection.ConnectAsync(options);
    await using var st = await value.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);
    await st.WriteAsync(Encoding.UTF8.GetBytes("hello\n"));
    var reader = new StreamReader(st);
    Console.WriteLine(await reader.ReadLineAsync());
}
catch (Exception e)
{
    Console.WriteLine(e);
}


X509Certificate2 CreateSelfSignedCertificate()
{
    var ecdsa = ECDsa.Create();
    var certificateRequest = new CertificateRequest("CN=localhost", ecdsa, HashAlgorithmName.SHA256);
    certificateRequest.CertificateExtensions.Add(
        new X509BasicConstraintsExtension(
            certificateAuthority: false,
            hasPathLengthConstraint: false,
            pathLengthConstraint: 0,
            critical: true
        )
    );
    certificateRequest.CertificateExtensions.Add(
        new X509KeyUsageExtension(
            keyUsages:
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment |
                X509KeyUsageFlags.CrlSign | X509KeyUsageFlags.KeyCertSign,
            critical: false
        )
    );
    certificateRequest.CertificateExtensions.Add(
        new X509EnhancedKeyUsageExtension(
            new OidCollection {
                    new Oid("1.3.6.1.5.5.7.3.2"), // TLS Client auth
                    new Oid("1.3.6.1.5.5.7.3.1")  // TLS Server auth
            },
            false));

    certificateRequest.CertificateExtensions.Add(
        new X509SubjectKeyIdentifierExtension(
            key: certificateRequest.PublicKey,
            critical: false
        )
    );

    var sanBuilder = new SubjectAlternativeNameBuilder();
    sanBuilder.AddDnsName("localhost");
    certificateRequest.CertificateExtensions.Add(sanBuilder.Build());

    return certificateRequest.CreateSelfSigned(DateTimeOffset.Now.AddDays(-1), DateTimeOffset.Now.AddYears(5));
}