#include "mainwindow.h"
#include "ui_mainwindow.h"

#include <service/context.h>

MainWindow::MainWindow(QWidget* parent)
: QMainWindow(parent),
  ui(new Ui::MainWindow)
{
    ui->setupUi(this);

    context::getInstance().dispatcher().consume([this](const std::string& message){
        ui->textBrowser->append(QString{message.data()} + "\n");
    });
}

MainWindow::~MainWindow()
{
    delete ui;
}

void MainWindow::on_pushButton_clicked()
{
    context::getInstance().dispatcher().publish(ui->textEdit->toPlainText().toStdString());
    ui->textEdit->setPlainText("");
}
