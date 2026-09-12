package studying.withoutsolid;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

/**
 * Пример нарушения принципов SOLID: один класс одновременно формирует текст
 * отчета, сохраняет его на диск и «отправляет» по электронной почте.
 */
public class ReportService {

    private static final DateTimeFormatter DISPLAY_FORMATTER = DateTimeFormatter.ofPattern("yyyy-MM-dd HH:mm:ss");
    private static final DateTimeFormatter FILE_NAME_FORMATTER = DateTimeFormatter.ofPattern("yyyy-MM-dd_HH-mm-ss");
    private static final Path REPORTS_DIRECTORY = Path.of("reports");

    private String reportText;
    private LocalDateTime reportDateTime;

    /**
     * Формирует текст отчета о продажах на основе текущей даты и переданных показателей.
     *
     * @param carsSold        количество проданных автомобилей
     * @param motorcyclesSold количество проданных мотоциклов
     * @return сформированный текст отчета
     */
    public String generateReport(int carsSold, int motorcyclesSold) {
        reportDateTime = LocalDateTime.now();
        reportText = """
                Отчет о продажах
                Дата и время: %s
                Продано автомобилей: %d
                Продано мотоциклов: %d
                """.formatted(reportDateTime.format(DISPLAY_FORMATTER), carsSold, motorcyclesSold);
        return reportText;
    }

    /**
     * Сохраняет последний сформированный отчет в файл в каталоге {@code reports}.
     *
     * @throws IOException если не удалось создать каталог или записать файл
     */
    public void saveReport() throws IOException {
        Files.createDirectories(REPORTS_DIRECTORY);
        String fileName = "report-" + reportDateTime.format(FILE_NAME_FORMATTER) + ".txt";
        Files.writeString(REPORTS_DIRECTORY.resolve(fileName), reportText);
    }

    /**
     * Имитирует отправку последнего сформированного отчета на электронную почту получателя.
     *
     * @param email адрес электронной почты получателя
     */
    public void sendReport(String email) {
        System.out.printf("Отправка отчета на почту %s:%n%s%n", email, reportText);
    }
}
