# RemoteTaskManager
![image](https://github.com/XKaguya/RemoteTaskManager/assets/96401952/0c4b9222-9408-4937-8e4f-92ae62bc6adf)

RemoteTaskManager is a tool used to kill a specific process remotely. It is currently only intended for local network use. The program does not employ any encryption or security methods to protect the host's safety.

**Disclaimer:** It is not recommended for use beyond the local network.

## Main Features

- Kill processes remotely using `NTSD` or `Taskkill`.

## TODO

- Customize remote port. (Which already has left a var, but lazy to do.)
- Language switch (Which I think is not necessary.)
- More methods for force killing. (Maybe same as above.)
- Open without console window.
- And etc...

## A Note

This program is designed to save my free time, especially in situations where manual intervention is necessary (such as when a process cannot be terminated locally and requires external assistance). It may also be compatible with RDPInterceptor due to a bug in the last release version.

The code may not be optimal, but contributions to this repository are always welcome.
