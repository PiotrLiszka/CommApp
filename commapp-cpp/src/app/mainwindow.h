#ifndef MAINWINDOW_H
#define MAINWINDOW_H

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
    std::string _login;
    Ui::MainWindow* ui;
};

#endif // MAINWINDOW_H
