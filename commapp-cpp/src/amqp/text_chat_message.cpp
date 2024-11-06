#include "text_chat_message.h"

#include <rapidjson/writer.h>
#include <rapidjson/stringbuffer.h>


text_chat_message::text_chat_message(const std::string &document_str)
{
    _document.Parse(document_str.c_str());
}

text_chat_message::text_chat_message(rapidjson::Document &&document)
{
    _document = std::move(document);
}

text_chat_message::text_chat_message(const std::string &senderId, const std::string &datetime, const std::string &chat_message)
{
    _document.SetObject();
    rapidjson::Value senderId_value(rapidjson::kStringType);
    senderId_value.SetString(senderId.c_str(), static_cast<rapidjson::SizeType>(senderId.length()), _document.GetAllocator());
    _document.AddMember("senderId", senderId_value, _document.GetAllocator());

    rapidjson::Value datetime_value(rapidjson::kStringType);
    datetime_value.SetString(datetime.c_str(), static_cast<rapidjson::SizeType>(datetime.length()), _document.GetAllocator());
    _document.AddMember("datetime", datetime_value, _document.GetAllocator());

    rapidjson::Value chat_message_value(rapidjson::kStringType);
    chat_message_value.SetString(chat_message.c_str(), static_cast<rapidjson::SizeType>(chat_message.length()), _document.GetAllocator());
    _document.AddMember("message", chat_message_value, _document.GetAllocator());
}

std::string text_chat_message::get_chat_message() const
{
    assert(_document.HasMember("message"));
    assert(_document["message"].IsString());
    return _document["message"].GetString();
}

std::string text_chat_message::date_time() const
{
    assert(_document.HasMember("datetime"));
    assert(_document["datetime"].IsString());
    return _document["datetime"].GetString();
}

std::string text_chat_message::sender_id() const
{
    assert(_document.HasMember("senderId"));
    assert(_document["senderId"].IsString());
    return _document["senderId"].GetString();
}

text_chat_message::operator std::string() const
{
    rapidjson::StringBuffer buffer;
    rapidjson::Writer<rapidjson::StringBuffer> writer(buffer);
    _document.Accept(writer);
    return buffer.GetString();
}
