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

text_chat_message::operator std::string() const
{
    rapidjson::StringBuffer buffer;
    rapidjson::Writer<rapidjson::StringBuffer> writer(buffer);
    _document.Accept(writer);
    return buffer.GetString();
}
