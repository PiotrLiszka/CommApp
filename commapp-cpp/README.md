# CommApp-Cpp

## Introduction
Yet another cross-platform text exchange application written in c++20.

TODO
<!-- used dependencies etc... -->

## License

TODO

## Building CommApp-Cpp

### Supported Platforms

* Linux
* Microsoft Windows

### Building CommApp-Cpp from Scratch

Various build systems are supported due to usage of cmake.

#### Linux

##### Run cmake and build with make:

<!-- No submodules yet -->
<!-- $ git submodule init && git submodule update -->

```
  $ mkdir build && cd build
  $ cmake .. -G "Unix Makefiles"
  $ make -j
```

##### Run cmake and build with Ninja:

```
  $ mkdir build && cd build
  $ cmake .. -G Ninja
  $ ninja
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