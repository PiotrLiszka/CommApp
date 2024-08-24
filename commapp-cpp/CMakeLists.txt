cmake_minimum_required(VERSION 3.30)

project(CommApp-Cpp VERSION 1.0)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

if (MSVC)
    add_compile_options(/std:c++20)
    # add_compile_options(/W4 /std:c++latest)
    # set(CMAKE_CXX_FLAGS_DEBUG "/Od /Wall /std:c++latest /DSC_ASSERT_ENABLED")
    # set(CMAKE_CXX_FLAGS_RELEASE "/W0 /O2 /DNDEBUG")
else ()
#   add_compile_options(-std=c++20 -Wall -Wextra -pedantic -Wconversion -Wcast-align -Wunused -Wshadow -Wold-style-cast -lgcov)
#   set(CMAKE_CXX_FLAGS_DEBUG "-g -DSC_ASSERT_ENABLED")
#   set(CMAKE_CXX_FLAGS_RELEASE "-O3 -DNDEBUG")
endif ()

include_directories(${CMAKE_SOURCE_DIR})
add_executable(CommApp-Cpp main.cpp)
