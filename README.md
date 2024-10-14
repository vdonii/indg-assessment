# Image Service

AWS-based service which performs basic CRUD operations + Resizing of images

## Prerequisites
-  AWS SDK + CLI
- .NET 8 SDK

## Configuring AWS
```
aws configure
```
## Running Build

```
PS: ./build/build.ps1
```
## Running Deployment

```
PS: ./deploy/build.ps1 --awsAccessKey <access_key> --awsSecretKey <secret_key> --awsExecutionRole <execution_role>
```
