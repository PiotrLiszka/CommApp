#include "inputtextedit.h"

InputTextEdit::InputTextEdit(QWidget* parent) : QTextEdit(parent)
{
}

void InputTextEdit::keyPressEvent(QKeyEvent *event)
{
    if (event->keyCombination().key() == Qt::Key_Return &&
        event->keyCombination().keyboardModifiers().toInt() == Qt::ShiftModifier)
    {
        QTextEdit::keyPressEvent(event);
    }
    else if (event->key() == Qt::Key_Return)
    {
        _on_key_pressed_callback();
    }
    else
    {
        QTextEdit::keyPressEvent(event);
    }
}

void InputTextEdit::setEnterCallback(std::function<void ()> enter_callback)
{
    _on_key_pressed_callback = std::move(enter_callback);
}
