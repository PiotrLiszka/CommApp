cmake_minimum_required(VERSION 3.30)

project(CommApp-Cpp)

set(CMAKE_CXX_STANDARD 20)
set(CMAKE_CXX_STANDARD_REQUIRED ON)
set(CMAKE_CXX_FLAGS "-std=c++20 -Wall -Wextra -pedantic -Wconversion -Wcast-align -Wunused -Wshadow -Wold-style-cast -lgcov")
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
find_package(Boost 1.82.0 REQUIRED)
INCLUDE_DIRECTORIES(${Boost_INCLUDE_DIR})
# ==========================================


set(SOURCE_DIR ${CMAKE_CURRENT_SOURCE_DIR}/src)
set(SOURCE_FILES
    ${SOURCE_DIR}/logger.h
    ${SOURCE_DIR}/logger.cpp
    ${SOURCE_DIR}/service/service.h
    ${SOURCE_DIR}/service/service.cpp
    ${SOURCE_DIR}/amqp/channel.h
    ${SOURCE_DIR}/amqp/channel.cpp
    ${CMAKE_CURRENT_SOURCE_DIR}/main.cpp
)

add_executable(CommApp-Cpp ${SOURCE_FILES})

target_link_libraries(CommApp-Cpp PUBLIC amqpcpp ${Boost_LIBRARIES} pthread dl ssl)

target_include_directories(CommApp-Cpp PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${SOURCE_DIR}
)

target_precompile_headers(CommApp-Cpp PUBLIC ${SOURCE_DIR}/stdafx.h)
