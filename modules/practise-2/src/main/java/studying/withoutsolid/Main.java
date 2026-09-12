package studying.withoutsolid;

import java.io.IOException;

/**
 * Демонстрация работы {@link ReportService}, в котором формирование, сохранение
 * и отправка отчета намеренно смешаны в одном классе.
 */
public class Main {

    public static void main(String[] args) throws IOException {
        ReportService reportService = new ReportService();
        reportService.generateReport(10, 5);
        reportService.saveReport();
        reportService.sendReport("boss@company.com");
    }
}
