cmake_minimum_required(VERSION 3.30)

project(CommApp-Cpp)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_FLAGS "-std=c++98 -Wall -Wextra -pedantic -Wconversion -Wcast-align -Wunused -Wshadow -Wold-style-cast -lgcov")
set(CMAKE_EXE_LINKER_FLAGS ${CMAKE_CXX_FLAGS})


# =============== EXTERNALS ===============
set(EXTERNALS_DIR ${CMAKE_SOURCE_DIR}/externals)

# ---------------------
#        AMQP-CPP
# ---------------------
SET(AMQP-CPP_LINUX_TCP ON CACHE BOOL "AMQP-CPP Linux TCP Enabled")
include_directories(${EXTERNALS_DIR}/AMQP-CPP)
add_subdirectory(${EXTERNALS_DIR}/AMQP-CPP)
# set_target_properties(amqpcpp PROPERTIES AMQP-CPP_LINUX_TCP	ON)

# ---------------------
#        Boost
# ---------------------
find_package(Boost REQUIRED)
INCLUDE_DIRECTORIES(${Boost_INCLUDE_DIR})
# ==========================================


include_directories(${CMAKE_SOURCE_DIR})
add_executable(CommApp-Cpp main.cpp)
target_link_libraries(CommApp-Cpp PUBLIC amqpcpp ${Boost_LIBRARIES} pthread dl ssl)

