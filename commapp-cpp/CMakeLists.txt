cmake_minimum_required(VERSION 3.30)

project(CommApp-Cpp VERSION 0.0.1 LANGUAGES CXX)

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


# ---------------------
#        RapidJSON
# ---------------------
set(RAPIDJSON_DIR ${EXTERNALS_DIR}/rapidjson/include)
file(GLOB_RECURSE rapidjson_sources
     ${RAPIDJSON_DIR}/rapidjson/*.h
)
# ==========================================

set(SOURCE_DIR ${CMAKE_CURRENT_SOURCE_DIR}/src)
set(SOURCE_FILES
    ${SOURCE_DIR}/logger.h
    ${SOURCE_DIR}/logger.cpp
    ${SOURCE_DIR}/service/qservice.h
    ${SOURCE_DIR}/service/qservice.cpp
    ${SOURCE_DIR}/service/context.h
    ${SOURCE_DIR}/service/context.cpp
    ${SOURCE_DIR}/amqp/dispatcher.h
    ${SOURCE_DIR}/amqp/dispatcher.cpp
    ${SOURCE_DIR}/amqp/text_chat_message.h
    ${SOURCE_DIR}/amqp/text_chat_message.cpp
    ${SOURCE_DIR}/app/inputtextedit.h
    ${SOURCE_DIR}/app/inputtextedit.cpp
    ${SOURCE_DIR}/app/mainwindow.h
    ${SOURCE_DIR}/app/mainwindow.cpp
    ${SOURCE_DIR}/app/mainwindow.ui
    ${SOURCE_DIR}/app/main.cpp
    ${SOURCE_DIR}/app/login/login_dialog.h
    ${SOURCE_DIR}/app/login/login_dialog.cpp
    ${SOURCE_DIR}/app/login/login_dialog.ui
    ${SOURCE_DIR}/app/list_widget_delegate.h
    ${SOURCE_DIR}/app/list_widget_delegate.cpp
)

qt_add_executable(CommApp-Cpp ${SOURCE_FILES} ${rapidjson_sources})

target_link_libraries(CommApp-Cpp PUBLIC Qt6::Widgets amqpcpp ${Boost_LIBRARIES} pthread dl ssl)

target_include_directories(CommApp-Cpp PUBLIC
    ${CMAKE_CURRENT_SOURCE_DIR}
    ${SOURCE_DIR}
    ${SOURCE_DIR}/app
    ${RAPIDJSON_DIR}
)

target_precompile_headers(CommApp-Cpp PUBLIC ${SOURCE_DIR}/stdafx.h)

# set_target_properties(helloworld PROPERTIES
#     WIN32_EXECUTABLE ON
#     MACOSX_BUNDLE ON
# )
