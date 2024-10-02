#ifndef LIST_WIDGET_DELEGATE_H
#define LIST_WIDGET_DELEGATE_H

#include <QAbstractItemDelegate>
#include <QObject>
#include <QWidget>

class list_widget_delegate : public QAbstractItemDelegate
{
public:
    explicit list_widget_delegate(QObject* parent = nullptr);

    void paint(QPainter * painter, const QStyleOptionViewItem& option, const QModelIndex& index) const;
    QSize sizeHint(const QStyleOptionViewItem& option, const QModelIndex& index) const;
};

#endif // LIST_WIDGET_DELEGATE_H
