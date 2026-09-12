package studying.withsolid.service.impl;

import studying.withsolid.exception.ApplicationErrorCode;
import studying.withsolid.exception.ApplicationException;
import studying.withsolid.model.Report;
import studying.withsolid.service.ReportSender;

/**
 * Реализация {@link ReportSender}, выводящая отчет и адрес получателя в консоль,
 * имитируя отправку электронного письма.
 */
public class ReportSenderImpl implements ReportSender {

    /**
     * Печатает текст отчета и адрес электронной почты получателя в консоль.
     *
     * @param report отчет для отправки
     * @param email  адрес электронной почты получателя
     * @throws ApplicationException с кодом {@link ApplicationErrorCode#VALIDATION_ERROR},
     *                              если отчет не задан или адрес электронной почты пуст
     */
    @Override
    public void send(Report report, String email) {
        if (report == null) {
            throw new ApplicationException(ApplicationErrorCode.VALIDATION_ERROR, "Отчет не должен быть null");
        }
        if (email == null || email.isBlank()) {
            throw new ApplicationException(ApplicationErrorCode.VALIDATION_ERROR,
                    "Email получателя не должен быть пустым");
        }
        System.out.printf("Отправка отчета на почту %s:%n%s%n", email, report.toReportText());
    }
}
