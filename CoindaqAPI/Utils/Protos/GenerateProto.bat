@rem enter this directory
cd /d %~dp0

set TOOLS_PATH=D:\InstallationSoftware\Protoc-3.5.1-win32\csharp

echo "Generate Csharp Class:"
%TOOLS_PATH%\protoc.exe -I=. common.proto kyc.proto two_factor.proto project.proto user.proto uwallet.proto otc.proto autojob.proto --csharp_out=..\..\Model --grpc_out=..\..\Model --plugin=protoc-gen-grpc=%TOOLS_PATH%\grpc_csharp_plugin.exe

echo "Generate Html Document:"
%TOOLS_PATH%\protoc.exe --doc_out=..\..\wwwroot --doc_opt=html,index.html common.proto kyc.proto two_factor.proto project.proto user.proto uwallet.proto otc.proto autojob.proto

echo "Complete proto class generation."