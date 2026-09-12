package studying.withsolid.service;

import studying.withsolid.exception.ApplicationException;
import studying.withsolid.model.Report;

/**
 * Отвечает за отправку отчета получателю.
 */
public interface ReportSender {

    /**
     * Отправляет отчет на указанный адрес электронной почты.
     *
     * @param report отчет для отправки
     * @param email  адрес электронной почты получателя
     * @throws ApplicationException если отчет или адрес электронной почты не заданы
     */
    void send(Report report, String email);
}
