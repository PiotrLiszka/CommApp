#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <service/context.h>
#include <login/login_dialog.h>

#include <QListWidget>
#include <QLabel>

MainWindow::MainWindow(QWidget* parent)
: QMainWindow(parent),
  ui(new Ui::MainWindow)
{
    ui->setupUi(this);
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::init()
{

    context::getInstance().dispatcher().consume([this](const std::string& message){
        if (ui->tabWidget->currentWidget() == nullptr)
        {
            qWarning() << "Missed message! " + message;
            return;
        }
        QListWidget* message_list_widget = static_cast<QListWidget*>(ui->tabWidget->currentWidget()); // use the user from message and open(possibly) tab
        message_list_widget->addItem(QString{message.c_str()});
    });
}

void MainWindow::on_pushButton_clicked()
{
    if (ui->tabWidget->currentWidget() == nullptr)
        return;

    const QString recipent_name = ui->tabWidget->tabText(ui->tabWidget->currentIndex());
    const QString to_be_send_text = ui->textEdit->toPlainText();
    if (to_be_send_text.isEmpty() || recipent_name.isEmpty())
        return;
    context::getInstance().dispatcher().publish(to_be_send_text.toStdString(), recipent_name.toStdString());
    QListWidget* message_list_widget = static_cast<QListWidget*>(ui->tabWidget->currentWidget()); // use the user from message and open(possibly) tab
    QListWidgetItem* this_user_message_item = new QListWidgetItem(to_be_send_text, message_list_widget);
    this_user_message_item->setTextAlignment(Qt::AlignRight);
    message_list_widget->addItem(this_user_message_item);
    ui->textEdit->setPlainText("");
}

void MainWindow::on_addNewFriendPushButton_clicked()
{
    const QString new_friend_str = ui->newFriendLineEdit->text();
    if (new_friend_str.isEmpty() || !(ui->friendsListWidget->findItems(new_friend_str, Qt::MatchExactly).isEmpty()))
        return;
    ui->friendsListWidget->addItem(new_friend_str);
    ui->newFriendLineEdit->clear();
}


void MainWindow::on_friendsListWidget_itemDoubleClicked(QListWidgetItem *item)
{
    const QString clicked_friend_name = item->text();
    for (int i = 0; i < ui->tabWidget->count(); i++)
    {
        if (ui->tabWidget->tabText(i) == clicked_friend_name)
            return;
    }
    QListWidget* new_qlist_widget = new QListWidget();
    ui->tabWidget->addTab(new_qlist_widget, item->text());
}

