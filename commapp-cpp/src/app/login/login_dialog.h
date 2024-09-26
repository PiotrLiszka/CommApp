#ifndef LOGIN_DIALOG_H
#define LOGIN_DIALOG_H

#include <QDialog>

namespace Ui
{
class login_dialog;
} // namespace Ui

class login_dialog : public QDialog
{
    Q_OBJECT

public:
    explicit login_dialog(QWidget* parent = nullptr);
    ~login_dialog();

    void set_when_accepted(std::function<void(const QString&)> when_accepted);
    void set_when_rejected(std::function<void()> when_rejected);

private slots:
    void on_loginDialogButtons_rejected();
    void on_loginDialogButtons_accepted();

private:
    Ui::login_dialog* ui;
    std::function<void(const QString&)> _when_accepted;
    std::function<void()> _when_rejected;
};

#endif // LOGIN_DIALOG_H
