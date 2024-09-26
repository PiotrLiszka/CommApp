#ifndef MESSAGE_H
#define MESSAGE_H

#include <rapidjson/document.h>

class text_chat_message
{
public:
    text_chat_message(const std::string& document_str);
    text_chat_message(rapidjson::Document&& document);
    text_chat_message(const std::string& senderId, const std::string& datetime, const std::string& chat_message);


    operator std::string() const;

private:
    rapidjson::Document _document;
};

#endif // MESSAGE_H
