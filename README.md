# Lighter Linux Signer Testing
This project can be run on either Windows or Linux, provided the requred signer file is provided.

## Setup

- Before running tests, please add the file **`local.test.json`** and populate it with your API key details. For an example of the required format please see **`local.test.mock.json`**.

## Signer File Naming

- **Linux** will use the signer named  `lighter-signer-linux.so`

- **Windows** will use the signer named `lighter-signer-windows-amd64.dll`

You can update these name by changing DLLName in LighterNativeWindows.cs and LighterNativeLinux.cs. To test different versions of the signer you can simply replace the current signer file with yours.

## Notes

- In our testing, the **Linux signer crashes consistently**.  
- The **Windows signer has never crashed**.