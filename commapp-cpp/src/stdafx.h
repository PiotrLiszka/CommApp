#pragma once

#include <cstdlib>
#include <exception>
#include <functional>
#include <memory>
#include <queue>
#include <string>
#include <thread>
#include <type_traits>
#include <utility>

#include <boost/asio/deadline_timer.hpp>
#include <boost/asio/io_service.hpp>
#include <boost/asio/signal_set.hpp>
#include <boost/asio/strand.hpp>
#include <boost/system/error_code.hpp>

#include <amqpcpp.h>
#include <amqpcpp/libboostasio.h>

#include "logger.h"