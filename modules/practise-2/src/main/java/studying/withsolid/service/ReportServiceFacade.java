package studying.withsolid.service;

import studying.withsolid.exception.ApplicationErrorCode;
import studying.withsolid.exception.ApplicationException;
import studying.withsolid.model.Report;

/**
 * Фасад, координирующий сохранение и отправку отчета через переданные
 * ему реализации {@link ReportSaver} и {@link ReportSender}.
 */
public class ReportServiceFacade {

    private final ReportSaver reportSaver;
    private final ReportSender reportSender;

    /**
     * Создает фасад с внедренными зависимостями.
     *
     * @param reportSaver  реализация сохранения отчета
     * @param reportSender реализация отправки отчета
     */
    public ReportServiceFacade(ReportSaver reportSaver, ReportSender reportSender) {
        this.reportSaver = reportSaver;
        this.reportSender = reportSender;
    }

    /**
     * Валидирует переданные данные, затем сохраняет отчет и отправляет его на почту получателя.
     *
     * @param report отчет для обработки
     * @param email  адрес электронной почты получателя
     * @throws ApplicationException с кодом {@link ApplicationErrorCode#VALIDATION_ERROR},
     *                              если отчет не задан или адрес электронной почты пуст
     */
    public void process(Report report, String email) {
        if (report == null) {
            throw new ApplicationException(ApplicationErrorCode.VALIDATION_ERROR, "Отчет не должен быть null");
        }
        if (email == null || email.isBlank()) {
            throw new ApplicationException(ApplicationErrorCode.VALIDATION_ERROR,
                    "Email получателя не должен быть пустым");
        }
        reportSaver.save(report);
        reportSender.send(report, email);
    }
}
