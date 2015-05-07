FROM anaerobic/fsharp-runtime-nuget

RUN mono nuget.exe restore
RUN xbuild /p:BuildWithMono="true" FSharpPostgresPusher.sln

CMD []
ENTRYPOINT ["mono", "FSharpPostgresPusher/bin/Debug/FSharpPostgresPusher.exe"]
