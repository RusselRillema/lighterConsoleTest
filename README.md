# Lighter Linux Signer Testing
This project can be run on either Windows or Linux, your os will be automatically detected.
## Setup

- Before running tests, please add the file **`local.test.json`** and populate it with your API key details. For an example of the required format please see **`local.test.mock.json`**. Alternatively if this is not done the app will promt you for the required fields and generate a **`local.test.json`** for you.

## Signer File Naming

- **Linux** will use the signer named  `lighter-signer-linux.so`

- **Windows** will use the signer named `lighter-signer-windows-amd64.dll`

To test different versions of the signer you can simply replace the current signer file with yours.
(If you want to update these names, you need to update DLLName in LighterNativeWindows.cs and LighterNativeLinux.cs as well as the names in LighterTest.csproj.)
## Notes

- In our testing, the **Linux signer crashes consistently**.
- The **Windows signer has never crashed**.