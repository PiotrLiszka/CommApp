#ifndef INPUTTEXTEDIT_H
#define INPUTTEXTEDIT_H

#include <QTextEdit>
#include <QKeyEvent>

class InputTextEdit : public QTextEdit
{
public:
    InputTextEdit(QWidget* parent = nullptr);

    virtual void keyPressEvent(QKeyEvent *event) override;
    void setEnterCallback(std::function<void()> enter_callback);

private:
    std::function<void()> _on_key_pressed_callback;
};

#endif // INPUTTEXTEDIT_H
