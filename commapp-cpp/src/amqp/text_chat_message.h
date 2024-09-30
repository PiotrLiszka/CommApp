#ifndef MESSAGE_H
#define MESSAGE_H

#include <rapidjson/document.h>

class text_chat_message
{
public:
    text_chat_message(const std::string& document_str);
    text_chat_message(rapidjson::Document&& document);
    text_chat_message(const std::string& senderId, const std::string& datetime, const std::string& chat_message);

    std::string get_chat_message() const;
    std::string date_time() const;
    std::string sender_id() const;
    operator std::string() const;

private:
    rapidjson::Document _document;
};

#endif // MESSAGE_H
