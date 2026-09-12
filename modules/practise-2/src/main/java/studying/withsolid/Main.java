package studying.withsolid;

import studying.withsolid.model.Report;
import studying.withsolid.service.ReportServiceFacade;
import studying.withsolid.service.impl.ReportSaverImpl;
import studying.withsolid.service.impl.ReportSenderImpl;

import java.time.LocalDateTime;

/**
 * Демонстрация SOLID-версии сервиса отчетов: реализации {@link ReportSaverImpl}
 * и {@link ReportSenderImpl} внедряются в {@link ReportServiceFacade} через конструктор.
 */
public class Main {

    public static void main(String[] args) {
        Report report = new Report(LocalDateTime.now(), 10, 5);
        ReportServiceFacade reportServiceFacade = new ReportServiceFacade(new ReportSaverImpl(), new ReportSenderImpl());
        reportServiceFacade.process(report, "boss@company.com");
    }
}
