# CommApp-Cpp

## Introduction
Yet another text exchange application written in c++20.

TODO
<!-- used dependencies etc... -->

## License

TODO

## Building CommApp-Cpp

### Supported Platforms

* Linux

### Building CommApp-Cpp from Scratch

Various build systems are supported due to usage of cmake.

#### Linux


```
  $ sudo apt-get install libboost-all-dev
```


```
  $ git submodule update --init --recursive
```


##### Run cmake and build with make:

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