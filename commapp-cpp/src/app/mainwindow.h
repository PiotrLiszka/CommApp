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

    void init();

private slots:
    void on_pushButton_clicked();
    void on_addNewFriendPushButton_clicked();
    void on_friendsListWidget_itemDoubleClicked(QListWidgetItem *item);

private:
    Ui::MainWindow* ui;
};

#endif // MAINWINDOW_H
