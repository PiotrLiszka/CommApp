#include "login_dialog.h"
#include "ui_login_dialog.h"

login_dialog::login_dialog(QWidget* parent)
: QDialog(parent),
  ui(new Ui::login_dialog)
{
    ui->setupUi(this);
}

login_dialog::~login_dialog()
{
    delete ui;
}

void login_dialog::set_when_accepted(std::function<void (const QString&)> when_accepted)
{
    _when_accepted = when_accepted;
}

void login_dialog::set_when_rejected(std::function<void ()> when_rejected)
{
    _when_rejected = when_rejected;
}

void login_dialog::on_loginDialogButtons_rejected()
{
    _when_rejected();
}


void login_dialog::on_loginDialogButtons_accepted()
{
    _when_accepted(ui->lineEdit_login->text());
}

