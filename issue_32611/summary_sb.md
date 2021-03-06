|                      | Win                                              | WSL                                              |
|----------------------|--------------------------------------------------|--------------------------------------------------|
| mono                 | My hostname is 'MAPICHOV-SB2'                    | My hostname is 'MAPICHOV-SB2'                    | 
|                      | My ip addresses are:                             | My ip addresses are:                             |
|                      |   fe80::fd0b:d2c4:b461:9949                      |   172.18.90.72                                   |
|                      |   2a00:5c20:101:10a::5                           |                                                  |
|                      |   192.168.0.109                                  |                                                  |
|                      |   fe80::b09c:2021:9ff7:c5b3                      |                                                  |
|                      |   fe80::6d8f:a006:8eae:458e                      |                                                  |
|                      |   fe80::418a:9295:429b:2555                      |                                                  |
|                      |   fe80::1:a8d3                                   |                                                  |
|                      |   2a01:110:10:40b::1:a8d3                        |                                                  |
|                      |   10.0.75.1                                      |                                                  |
|                      |   172.18.80.1                                    |                                                  |
|                      |   172.21.240.1                                   |                                                  |
|                      |   100.64.21.91                                   |                                                  |
| framework48          | My hostname is 'MAPICHOV-SB2'                    |                                                  | 
|                      | My ip addresses are:                             |                                                  |
|                      |   fe80::fd0b:d2c4:b461:9949%14                   |                                                  |
|                      |   2a00:5c20:101:10a::5                           |                                                  |
|                      |   192.168.0.109                                  |                                                  |
|                      |   fe80::b09c:2021:9ff7:c5b3%23                   |                                                  |
|                      |   fe80::6d8f:a006:8eae:458e%58                   |                                                  |
|                      |   fe80::418a:9295:429b:2555%29                   |                                                  |
|                      |   fe80::1:a8d3%27                                |                                                  |
|                      |   2a01:110:10:40b::1:a8d3                        |                                                  |
|                      |   10.0.75.1                                      |                                                  |
|                      |   172.18.80.1                                    |                                                  |
|                      |   172.21.240.1                                   |                                                  |
|                      |   100.64.21.91                                   |                                                  |
| dotnet core          | My hostname is 'MAPICHOV-SB2'                    | My hostname is 'MAPICHOV-SB2'                    | 
|                      | My ip addresses are:                             | My ip addresses are:                             |
|                      |   fe80::fd0b:d2c4:b461:9949%14    wifi           |   127.0.1.1                                      |
|                      |   2a00:5c20:101:10a::5            wifi           |                                                  |
|                      |   192.168.0.109                   wifi           |                                                  |
|                      |   fe80::b09c:2021:9ff7:c5b3%23    docker         |                                                  |
|                      |   fe80::6d8f:a006:8eae:458e%58    wsl            |                                                  |
|                      |   fe80::418a:9295:429b:2555%29    def            |                                                  |
|                      |   fe80::1:a8d3%27                 ms vpn         |                                                  |
|                      |   2a01:110:10:40b::1:a8d3         ms vpn         |                                                  |
|                      |   10.0.75.1                       docker         |                                                  |
|                      |   172.18.80.1                     wsl            |                                                  |
|                      |   172.21.240.1                    def            |                                                  |
|                      |   100.64.21.91                    ms vpn         |                                                  |
| getaddrinfo          | fe80::fd0b:d2c4:b461:9949                        |   127.0.1.1 (3x)                                 |
|                      | 2a00:5c20:101:10a::5                             |                                                  |
|                      | 192.168.0.109                                    |                                                  |
|                      | fe80::b09c:2021:9ff7:c5b3                        |                                                  |
|                      | fe80::6d8f:a006:8eae:458e                        |                                                  |
|                      | fe80::418a:9295:429b:2555                        |                                                  |
|                      | fe80::1:a8d3                                     |                                                  |
|                      | 2a01:110:10:40b::1:a8d3                          |                                                  |
|                      | 10.0.75.1                                        |                                                  |
|                      | 172.18.80.1                                      |                                                  |
|                      | 172.21.240.1                                     |                                                  |
|                      | 100.64.21.91                                     |                                                  |
| getifaddrs/          | fe80::418a:9295:429b:2555%29 (Default Switch)    | 127.0.0.1 (lo)                                   |
| GetAdaptersAddresses | 172.21.240.1 (Default Switch)                    | 172.18.90.72 (eth0)                              |
|                      | fe80::6d8f:a006:8eae:458e%58 (WSL)               | ::1 (lo)                                         |
|                      | 172.18.80.1 (WSL)                                | fe80::215:5dff:fee2:ad1e%eth0 (eth0)             |
|                      | fe80::b09c:2021:9ff7:c5b3%23 (DockerNAT)         |                                                  |
|                      | 10.0.75.1 (DockerNAT)                            |                                                  |
|                      | fe80::4850:1003:53a1:25b2%17 (Local Area Con1)   |                                                  |
|                      | 169.254.37.178 (Local Area Con1)                 |                                                  |
|                      | fe80::581b:f2a4:d661:a637%13 (Local Area Con2)   |                                                  |
|                      | 169.254.166.55 (Local Area Con2)                 |                                                  |
|                      | 2a00:5c20:101:10a::5 (Wi-Fi)                     |                                                  |
|                      | fe80::fd0b:d2c4:b461:9949%14 (Wi-Fi)             |                                                  |
|                      | 192.168.0.109 (Wi-Fi)                            |                                                  |
|                      | 2a01:110:10:40b::1:a8d3 (MSFTVPN)                |                                                  |
|                      | fe80::1:a8d3%27 (MSFTVPN)                        |                                                  |
|                      | 100.64.21.91 (MSFTVPN)                           |                                                  |
|                      | ::1 (Loopback)                                   |                                                  |
|                      | 127.0.0.1(Loopback)                              |                                                  |