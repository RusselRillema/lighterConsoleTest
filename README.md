# Lighter Linux Signer Testing

This project can be run on either Windows or Linux, provided the requred signer file is provided.

## Setup

- Before running tests, populate **`local.test.json`** with your API key details. Please ensure this file is located where the code will be executed.  
- For example, if testing in **Debug** mode, place the signer file in: /bin/Debug/net9.0
- For an example of the format please see **`local.test.mock.json`**.

## Signer File Naming

- **Linux**: The signer file must be named  
  `lighter-signer-linux.so`

- **Windows**: The signer file must be named  
  `lighter-signer-windows-amd64.dll`

## Placement

Add the signer file to the directory where the code will be executed.  
For example, if testing in **Debug** mode, place the signer file in: /bin/Debug/net9.0

## Notes

- In our testing, the **Linux signer crashes consistently**.  
- The **Windows signer has never crashed**.