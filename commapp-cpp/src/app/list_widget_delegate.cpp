#include "list_widget_delegate.h"

#include <QPainter>
#include <QStyleOptionViewItem>
#include <QPainterPath>
#include <QPen>
#include <QFontMetrics>
#include <QRgb>


list_widget_delegate::list_widget_delegate(QObject* parent)
: QAbstractItemDelegate{parent}
{
}

void list_widget_delegate::paint(QPainter *painter, const QStyleOptionViewItem &option, const QModelIndex &index) const
{
    painter->setRenderHint(QPainter::TextAntialiasing);
    painter->setRenderHint(QPainter::Antialiasing);
    const QPen original_pen = painter->pen();

    const Qt::AlignmentFlag alignment = static_cast<Qt::AlignmentFlag>(index.data(Qt::TextAlignmentRole).toInt());
    const QString text = index.data(Qt::DisplayRole).toString();

    if (alignment == Qt::AlignCenter)
    {
        painter->setFont(QFont("Lucida Grande", 7, QFont::Normal));
        painter->drawText(option.rect, alignment, text);
        return;
    }

    painter->setFont(QFont( "Lucida Grande", 12, QFont::Normal));

    const QStringList multiline_text = text.split('\n');

    int max_line_width = 0;
    for (const QString& line : multiline_text)
    {
        if (int line_width = painter->fontMetrics().horizontalAdvance(line); line_width > max_line_width)
        {
            max_line_width = line_width;
        }
    }

    constexpr int text_width_offset = 6;
    int width = max_line_width + text_width_offset;

    QRect chat_cloud_rect = option.rect;
    QRect chat_text_rect = option.rect;

    QPen text_pen = painter->pen();
    QColor top_gradiend_color;
    QColor bottom_gradiend_color;
    if (alignment == Qt::AlignLeft)
    {
        top_gradiend_color.setRgb(240, 240, 240);
        bottom_gradiend_color.setRgb(240, 240, 240);
        text_pen.setColor(Qt::black);
        chat_cloud_rect.setLeft(chat_cloud_rect.left());
        chat_cloud_rect.setRight(chat_cloud_rect.left() + width + text_width_offset);
        chat_text_rect.setLeft(chat_text_rect.left() + text_width_offset);
    }
    else if (alignment == Qt::AlignRight)
    {
        top_gradiend_color.setRgb(0, 198, 255);
        bottom_gradiend_color.setRgb(0, 120, 255);
        text_pen.setColor(Qt::white);
        chat_cloud_rect.setRight(chat_cloud_rect.right());
        chat_cloud_rect.setLeft(chat_cloud_rect.right() - width - text_width_offset);
        chat_text_rect.setRight(chat_text_rect.right() - text_width_offset);
    }
    chat_text_rect.setTop(chat_text_rect.top() + 2);

    {
        // draw chat cloud
        QPainterPath path;
        path.addRoundedRect(chat_cloud_rect, 8, 8);
        QLinearGradient gradient(chat_text_rect.topLeft(), chat_text_rect.bottomLeft());
        gradient.setColorAt(0, top_gradiend_color);
        gradient.setColorAt(1, bottom_gradiend_color);
        painter->fillPath(path, gradient);
    }

    painter->setPen(text_pen);
    painter->drawText(chat_text_rect, alignment, text);
    painter->setPen(original_pen);
}

QSize list_widget_delegate::sizeHint(const QStyleOptionViewItem &/*option*/, const QModelIndex &index) const
{
    const Qt::AlignmentFlag alignment = static_cast<Qt::AlignmentFlag>(index.data(Qt::TextAlignmentRole).toInt());
    const QString text = index.data(Qt::DisplayRole).toString();
    if (alignment == Qt::AlignCenter)
    {
        // width is autmated due to fixed(sort of) width QListWidget
        QFont font =  QFont( "Lucida Grande", 7, QFont::Normal);
        QFontMetrics fm(font);
        return QSize(1, fm.height());
    }


    const int breaklines_count = static_cast<int>(text.count('\n')) + 1;
    QFont font =  QFont( "Lucida Grande", 12, QFont::Normal);
    QFontMetrics fm(font);

    // width is autmated due to fixed(sort of) QListWidget
    int margin = 5;
    return QSize(1, fm.height() * breaklines_count + margin);
}
