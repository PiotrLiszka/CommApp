#ifndef MAINWINDOW_H
#define MAINWINDOW_H

#include <QMainWindow>
#include <service/service.h>

namespace Ui
{
class MainWindow;
} // namespace Ui

class MainWindow : public QMainWindow
{
    Q_OBJECT

public:
    explicit MainWindow(QWidget* parent, service::basic_service& basic_service);
    ~MainWindow();

private slots:
    void on_pushButton_clicked();

private:
    Ui::MainWindow* ui;
    service::basic_service& service;
};

#endif // MAINWINDOW_H
