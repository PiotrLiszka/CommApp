# CommApp-Cpp

## Introduction
Yet another text exchange application written in c++ for linux(so far).

TODO
// used dependencies etc...

## License

TODO

## Building CommApp-Cpp

### Supported Platforms

* Linux
* Microsoft Windows

### Building CommApp-Cpp from Scratch

TODO
General things

#### Linux

TODO

Run cmake and build with make:

```
  $ git submodule init && git submodule update
  $ mkdir build && cd build
  $ cmake ..
  $ make -j
```

You can also specify the build type:

```
  $ cmake -DCMAKE_BUILD_TYPE=Debug ..
```
  or
```
  $ cmake -DCMAKE_BUILD_TYPE=Release ..
```

#### Windows

##### Generate Visual Studio build system and build project

Generate Visual Studio build system
```
mkdir build
cd build
cmake .. -G "Visual Studio 17 2022"
```

Build using Developer Command Prompt
```
msbuild CommApp-Cpp.sln -t:Build
```
Run
CommApp-Cpp.exe