#pragma once

#include <exception>
#include <functional>
#include <memory>
#include <string>
#include <thread>
#include <utility>

#include <boost/asio/deadline_timer.hpp>
#include <boost/asio/io_service.hpp>
#include <boost/asio/strand.hpp>

#include <amqpcpp.h>
#include <amqpcpp/libboostasio.h>

#include "logger.h"