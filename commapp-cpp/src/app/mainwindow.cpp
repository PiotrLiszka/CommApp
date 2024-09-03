#include "mainwindow.h"
#include "ui_mainwindow.h"

MainWindow::MainWindow(QWidget* parent, service::basic_service& basic_service)
: QMainWindow(parent),
  ui(new Ui::MainWindow),
  service(basic_service)
{
    ui->setupUi(this);
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::on_pushButton_clicked()
{
    service.publish(ui->textEdit->toPlainText().toStdString());
    ui->textEdit->setPlainText("");

}
