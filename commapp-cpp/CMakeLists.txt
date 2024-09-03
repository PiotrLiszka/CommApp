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
# ==========================================


# ---------------------
#        Boost
# ---------------------
find_package(Boost 1.82.0 REQUIRED)
INCLUDE_DIRECTORIES(${Boost_INCLUDE_DIR})
# ==========================================


# ---------------------
#        QT
# ---------------------
find_package(Qt6 REQUIRED COMPONENTS Widgets)
qt_standard_project_setup()
# cmake -DCMAKE_PREFIX_PATH=/home/marcin/Qt/6.7.2/gcc_64 .. -G Ninja
# ==========================================

set(SOURCE_DIR ${CMAKE_CURRENT_SOURCE_DIR}/src)
set(SOURCE_FILES
    ${SOURCE_DIR}/logger.h
    ${SOURCE_DIR}/logger.cpp
    ${SOURCE_DIR}/service/service.h
    ${SOURCE_DIR}/service/service.cpp
    ${SOURCE_DIR}/amqp/channel.h
    ${SOURCE_DIR}/amqp/channel.cpp
    ${SOURCE_DIR}/app/mainwindow.h
    ${SOURCE_DIR}/app/mainwindow.cpp
    ${SOURCE_DIR}/app/mainwindow.ui
    ${SOURCE_DIR}/app/main.cpp
)

qt_add_executable(CommApp-Cpp ${SOURCE_FILES}
    src/app/mainwindow.h src/app/mainwindow.cpp src/app/mainwindow.ui

)

target_link_libraries(CommApp-Cpp PUBLIC Qt6::Widgets amqpcpp ${Boost_LIBRARIES} pthread dl ssl)

target_include_directories(CommApp-Cpp PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${SOURCE_DIR}
)

target_precompile_headers(CommApp-Cpp PUBLIC ${SOURCE_DIR}/stdafx.h)

# set_target_properties(helloworld PROPERTIES
#     WIN32_EXECUTABLE ON
#     MACOSX_BUNDLE ON
# )
