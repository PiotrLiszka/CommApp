#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <service/context.h>
#include <login/login_dialog.h>
#include <amqp/text_chat_message.h>

#include <QListWidget>
#include <QLabel>

namespace {
std::string utcTimeNow()
{
    std::time_t time = std::time({});
    char timeString[std::size("yyyy-mm-ddThh:mm:ssZ")];
    std::strftime(std::data(timeString), std::size(timeString),
                  "%FT%TZ", std::gmtime(&time));
    return timeString;
}
}

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

void MainWindow::init(const std::string &login)
{
    _login = login;

    context::getInstance().dispatcher().consume([this](const std::string& message){
        if (ui->tabWidget->currentWidget() == nullptr)
        {
            qWarning() << "Missed message! " + message;
            return;
        }
        QListWidget* message_list_widget = static_cast<QListWidget*>(ui->tabWidget->currentWidget());
        message_list_widget->addItem(QString{message.c_str()});
    });

    ui->textEdit->setEnterCallback(std::bind(&MainWindow::on_pushButton_clicked, this));
}

void MainWindow::on_pushButton_clicked()
{
    if (ui->tabWidget->currentWidget() == nullptr)
        return;

    const QString recipent_name = ui->tabWidget->tabText(ui->tabWidget->currentIndex());
    const QString to_be_send_text = ui->textEdit->toPlainText();
    if (to_be_send_text.isEmpty() || recipent_name.isEmpty())
        return;


    text_chat_message msg(_login, utcTimeNow(), to_be_send_text.toStdString());
    context::getInstance().dispatcher().publish(msg, recipent_name.toStdString());

    QListWidget* message_list_widget = static_cast<QListWidget*>(ui->tabWidget->currentWidget());

    // font
    QFont qlist_widget_font = message_list_widget->font();
    qlist_widget_font.setFamily("lucida grande");
    qlist_widget_font.setPointSize(13);
    message_list_widget->setFont(qlist_widget_font);

    QListWidgetItem* this_user_message_item = new QListWidgetItem(to_be_send_text, message_list_widget);
    QLinearGradient gradient(0, 0, 0, this_user_message_item->sizeHint().height());
    gradient.setColorAt(1, QColor{198, 242, 255});
    gradient.setColorAt(0, QColor{164, 204, 255});
    QBrush brush(gradient);

    this_user_message_item->setBackground(brush);
    this_user_message_item->setTextAlignment(Qt::AlignRight);

    message_list_widget->addItem(this_user_message_item);
    message_list_widget->scrollToBottom();

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

