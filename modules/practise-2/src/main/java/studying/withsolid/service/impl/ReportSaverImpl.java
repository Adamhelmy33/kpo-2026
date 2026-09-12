package studying.withsolid.service.impl;

import studying.withsolid.exception.ApplicationErrorCode;
import studying.withsolid.exception.ApplicationException;
import studying.withsolid.model.Report;
import studying.withsolid.service.ReportSaver;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.format.DateTimeFormatter;

/**
 * Реализация {@link ReportSaver}, сохраняющая отчет в текстовый файл на диске.
 */
public class ReportSaverImpl implements ReportSaver {

    private static final DateTimeFormatter FILE_NAME_FORMATTER = DateTimeFormatter.ofPattern("yyyy-MM-dd_HH-mm-ss");
    private static final Path DEFAULT_DIRECTORY = Path.of("reports");

    private final Path directory;

    /**
     * Создает сохранятель отчетов, использующий каталог {@code reports} по умолчанию.
     */
    public ReportSaverImpl() {
        this(DEFAULT_DIRECTORY);
    }

    /**
     * Создает сохранятель отчетов с внедренным каталогом для сохранения файлов.
     *
     * @param directory каталог, в который будут сохраняться отчеты
     */
    public ReportSaverImpl(Path directory) {
        this.directory = directory;
    }

    /**
     * Сохраняет отчет в файл {@code report-<yyyy-MM-dd_HH-mm-ss>.txt} внутри заданного каталога,
     * где дата и время берутся из даты формирования отчета.
     *
     * @param report отчет для сохранения
     * @throws ApplicationException с кодом {@link ApplicationErrorCode#FILE_WRITE_ERROR},
     *                              если во время записи файла произошла ошибка ввода-вывода
     */
    @Override
    public void save(Report report) {
        try {
            Files.createDirectories(directory);
            String fileName = "report-" + report.dateTime().format(FILE_NAME_FORMATTER) + ".txt";
            Files.writeString(directory.resolve(fileName), report.toReportText());
        } catch (IOException e) {
            throw new ApplicationException(ApplicationErrorCode.FILE_WRITE_ERROR,
                    "Не удалось сохранить отчет в файл", e);
        }
    }
}
