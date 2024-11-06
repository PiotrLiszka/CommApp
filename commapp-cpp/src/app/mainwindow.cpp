#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <service/context.h>
#include <login/login_dialog.h>
#include <amqp/text_chat_message.h>
#include "list_widget_delegate.h"

#include <chrono>

#include <QListWidget>
#include <QScrollBar>
#include <QMessageBox>
#include <QLabel>

namespace {

std::string utcTimeNow()
{
    return std::format("{0:%F}T{0:%R}Z", std::chrono::utc_clock::now());
}

QString local_time()
{
    const auto utc_offset = std::chrono::current_zone()->get_info(std::chrono::system_clock::now()).offset;
    return std::format("{0:%F} {0:%R}", std::chrono::utc_clock::now() + utc_offset).c_str();
}

QString utc_time_str_to_local_time_str(const std::string& utc_time_str)
{
    const auto utc_offset = std::chrono::current_zone()->get_info(std::chrono::system_clock::now()).offset;
    tm tm = {};
    std::istringstream ss(utc_time_str);
    ss >> std::get_time(&tm, "%Y-%m-%dT%R");
    std::chrono::system_clock::time_point time_point_from_str =
      std::chrono::system_clock::from_time_t(timegm(&tm)) + utc_offset;
    return std::format("{0:%F} {0:%R}", time_point_from_str).c_str();
}
}

MainWindow::MainWindow(QWidget* parent)
: QMainWindow(parent),
  ui(new Ui::MainWindow)
{
    ui->setupUi(this);
    ui->tabWidget->setTabsClosable(true);
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::init(const std::string &login)
{
    ui->usernameLabel->setText(login.c_str());

    context::getInstance().dispatcher().consume([this](const std::string& message){
        const text_chat_message incoming_message(message);
        QListWidget* message_list_widget = get_or_create_tab_list_widget(incoming_message.sender_id().c_str());
        add_message_to_widget(message_list_widget, incoming_message, Qt::AlignLeft);
        message_list_widget->scrollToBottom();
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


    text_chat_message msg(ui->usernameLabel->text().toStdString(), utcTimeNow(), to_be_send_text.toStdString());
    context::getInstance().dispatcher().publish(msg, recipent_name.toStdString());

    QListWidget* current_list_widget = get_current_list_widget();
    add_message_to_widget(current_list_widget, msg, Qt::AlignRight);

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
        {
            ui->tabWidget->setCurrentIndex(i);
            return;
        }
    }

    QListWidget* new_qlist_widget = create_new_list_widget(item->text());
    ui->tabWidget->setCurrentWidget(new_qlist_widget);
}

QListWidget *MainWindow::get_current_list_widget() const
{
    QListWidget* message_list_widget = static_cast<QListWidget*>(ui->tabWidget->currentWidget());
    assert(message_list_widget);
    return message_list_widget;
}
QListWidget *MainWindow::create_new_list_widget(const QString& tab_name)
{
    QListWidget* new_qlist_widget = new QListWidget();
    new_qlist_widget->setStyleSheet("""QListWidget{background: white;}""");
    new_qlist_widget->setSpacing(1);
    new_qlist_widget->setItemDelegate(new list_widget_delegate(new_qlist_widget));
    new_qlist_widget->setFlow(QListView::TopToBottom);
    const int new_tab_index = ui->tabWidget->addTab(new_qlist_widget, tab_name);
    connect(ui->tabWidget->tabBar(), &QTabBar::tabCloseRequested, [this, new_tab_index](){ ui->tabWidget->removeTab(new_tab_index); });
    return new_qlist_widget;
}


QListWidget *MainWindow::get_or_create_tab_list_widget(const QString &tab_name)
{
    for (int i = 0; i < ui->tabWidget->count(); i++)
    {
        if (ui->tabWidget->tabText(i) == tab_name)
        {
            return static_cast<QListWidget*>(ui->tabWidget->widget(i));
        }
    }

    return create_new_list_widget(tab_name);
}

void MainWindow::add_message_to_widget(QListWidget *target_widget, const text_chat_message &message, const Qt::AlignmentFlag alignment)
{
    if (target_widget->count() == 0 || target_widget->item(target_widget->count() - 1)->textAlignment() != alignment)
    {
        QString time = alignment == Qt::AlignRight ? local_time() : utc_time_str_to_local_time_str(message.date_time());
        QListWidgetItem* time_item = new QListWidgetItem(time, target_widget);
        time_item->setTextAlignment(Qt::AlignCenter);
        target_widget->addItem(time_item);
    }

    QListWidgetItem* this_user_message_item = new QListWidgetItem(message.get_chat_message().c_str(), target_widget);
    this_user_message_item->setTextAlignment(alignment);
    target_widget->addItem(this_user_message_item);
    target_widget->scrollToBottom();
}


