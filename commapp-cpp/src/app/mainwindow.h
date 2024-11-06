#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include "amqp/text_chat_message.h"
#include <QMainWindow>
#include <QListWidgetItem>

namespace Ui
{
class MainWindow;
} // namespace Ui

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget* parent = nullptr);
    ~MainWindow();

    void init(const std::string& login);

private slots:
    void on_pushButton_clicked();
    void on_addNewFriendPushButton_clicked();
    void on_friendsListWidget_itemDoubleClicked(QListWidgetItem *item);

private:
    Ui::MainWindow* ui;

    QListWidget* get_current_list_widget() const;
    QListWidget* create_new_list_widget(const QString &tab_name);
    QListWidget* get_or_create_tab_list_widget(const QString& tab_name);
    void add_message_to_widget(QListWidget* target_widget, const text_chat_message& message, const Qt::AlignmentFlag alignment);
};

#endif // MAINWINDOW_H
