# Network configuration of TCP/IP SQL Server in Virtual Machine (Linux Guest) and how to connect it in our App as (Linux Host)

Since some machines are not applicable with SQL Server we can try this alternative solution using Virtual machines and it is recommended to install the Linux OS only for better performance and less consume of memory.

**Linux Distros that is applicable to SQL Server:**
Red Hat Enterprise Linux, and Ubuntu

**CONNECTION STRUCTURE**
| VIRTUAL GUEST       |              HOST              |
|---------------------|--------------------------------|
| LINUX               | LINUX (MSSQL not applicable)   |
| LINUX               | WINDOWS (MSSQL not applicable) |


## SQL Server Installation in Linux (Ubuntu | GUEST) 
You must have an Ubuntu 20.04 machine with at least 2 GB of memory.

To install Ubuntu 20.04 on your own machine, go to https://releases.ubuntu.com/20.04/. You can also create Ubuntu virtual machines in Azure. See Create and Manage Linux VMs with the Azure CLI.

The Windows Subsystem for Linux isn't supported as an installation target for SQL Server.

For other system requirements, see System requirements for SQL Server on Linux.

1. Import the public repository GPG keys:
`$ wget -qO- https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -`

2. Register the SQL Server Ubuntu repository:
`$ sudo add-apt-repository "$(wget -qO- https://packages.microsoft.com/config/ubuntu/20.04/mssql-server-2019.list)"`

3. Run the following commands to install SQL Server:
`$ sudo apt-get update`
`$ sudo apt-get install -y mssql-server`

4. After the package installation finishes, run mssql-conf setup and follow the prompts to set the SA password and choose your edition. As a reminder, the following SQL Server editions are freely licensed: Evaluation, Developer, and Express.
`$ sudo /opt/mssql/bin/mssql-conf setup`

5. Once the configuration is done, verify that the service is running:
`$ systemctl status mssql-server --no-pager`

6. If you plan to connect remotely, you might also need to open the SQL Server TCP port (default 1433) on your firewall.

## Install the SQL Server command-line tools (HOST&GUEST)
To create a database, you need to connect with a tool that can run Transact-SQL statements on SQL Server. The following steps install the SQL Server command-line tools: `sqlcmd` and `bcp`.

Use the following steps to install the mssql-tools on Ubuntu. If curl isn't installed, you can run this code:
`$ sudo apt-get update`
`$ sudo apt install curl`

1. Import the public repository GPG keys.
`$ curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -`

2. Register the Ubuntu repository.
`$ curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list | sudo tee /etc/apt/sources.list.d/msprod.list`

3. Update the sources list and run the installation command with the unixODBC developer package. For more information, see Install the Microsoft ODBC driver for SQL Server (Linux).
`$ sudo apt-get update`
`$ sudo apt-get install mssql-tools unixodbc-dev`

You can update to the latest version of mssql-tools using the following commands:
`$ sudo apt-get update`
`$ sudo apt-get install mssql-tools`

For convenience, add symbolic link `/opt/mssql-tools/bin/` to `/usr/local/bin`  to make `sqlcmd` accessible from the bash shell.
`$ sudo ln -s /opt/mssql-tools/bin/ /usr/local/bin/`

#### **TRY TO CONNECT**
Run sqlcmd with parameters for your SQL Server name **-S**, the user name **-U**, and the password **-P**. In this tutorial, you are connecting locally, so the server name is localhost. The user name is **SA** and the password is the one you provided for the **SA** account during setup.

`$ sqlcmd -S localhost -U SA -P '<YourPassword>'`

Expected Ouput should be:
`1>` which is the sqlcmd command prompt where you can execute queries.

## SQL Server connection setup (GUEST)
#### Change the TCP port
The network.tcpport setting changes the TCP port where SQL Server listens for connections. By default, this port is set to 1433. To change the port, run the following commands:

1. Run the mssql-conf script as root with the "set" command for "network.tcpport":
`$ sudo /opt/mssql/bin/mssql-conf set network.tcpport <new_tcp_port>`

2. Restart the SQL Server service:
`$ sudo systemctl restart mssql-server`

3. When connecting to SQL Server now, you must specify the custom port with a comma (,) after the hostname or IP address. For example, to connect with SQLCMD, you would use the following command:
`$ sqlcmd -S localhost,<new_tcp_port> -U test -P test`

Note we are using a virtual machine make sure to change the settings of the network type into **Bridge** or **Host-only** so that we can ping the connection of virtual machine (Linux) to the host (Linux|Windows) because **NAT** will give us a private IP address.

To get our IP address under inet copy the IP address:

`$ ifconfig`

```
enp0s3: flags=4163<UP,BROADCAST,RUNNING,MULTICAST>  mtu 1500
        inet 192.168.1.8  netmask 255.255.255.0  broadcast 192.168.1.255
        inet6 fe80::b4b9:5fd9:d89f:e1f6  prefixlen 64  scopeid 0x20<link>
        ether 08:00:27:15:d9:89  txqueuelen 1000  (Ethernet)
        RX packets 68310  bytes 81663185 (81.6 MB)
        RX errors 0  dropped 3749  overruns 0  frame 0
        TX packets 12945  bytes 1770895 (1.7 MB)
        TX errors 0  dropped 0 overruns 0  carrier 0  collisions 0
```

Now since our TCP port is configured, we will check the listening ports by typing: 

`$ netstat -ln` to filter only listening TCP ports

```terminal
Active Internet connections (only servers)
Proto Recv-Q Send-Q Local Address           Foreign Address         State      
tcp        0      0 localhost:ipp           0.0.0.0:*               LISTEN     
tcp        0      0 localhost:domain        0.0.0.0:*               LISTEN     
tcp        0      0 localhost:44035         0.0.0.0:*               LISTEN     
tcp        0      0 0.0.0.0:4066            0.0.0.0:*               LISTEN     
tcp        0      0 localhost:ms-sql-m      0.0.0.0:*               LISTEN     
tcp        0      0 localhost:1431          0.0.0.0:*               LISTEN     
tcp6       0      0 ip6-localhost:ipp       [::]:*                  LISTEN     
tcp6       0      0 [::]:4066               [::]:*                  LISTEN     
tcp6       0      0 ip6-localhost:ms-sql-m  [::]:*                  LISTEN     
tcp6       0      0 ip6-localhost:1431      [::]:*                  LISTEN 
```

In my end I am using port 4066 and it is now listening for our connection out of the virtual box.

## Connecting the SQL Server into our App (HOST)
In the **appsettings.json** @jambrad is using the integrated security of Windows machine and we are using Linux with System Server Authentication.

Replace the SQL script properties to:

```json
"SqlConnection": "Server=tcp:192.168.1.8,4066;Database=CompanyDb;Integrated Security=false;User ID=SA;Password=*****;MultipleActiveResultSets=False;Encrypt=False;TrustServerCertificate=False;Connection Timeout=30;"
```

REFERENCES:

https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-ubuntu?view=sql-server-linux-ver15&preserve-view=true

https://stackoverflow.com/questions/34430550/a-connection-was-successfully-established-with-the-server-but-then-an-error-occ

https://docs.microsoft.com/en-us/sql/linux/sql-server-linux-configure-mssql-conf?view=sql-server-ver16#tcpport