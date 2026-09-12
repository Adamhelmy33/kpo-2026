package studying.withsolid.service.impl;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import org.junit.jupiter.api.io.TempDir;
import studying.withsolid.exception.ApplicationErrorCode;
import studying.withsolid.exception.ApplicationException;
import studying.withsolid.model.Report;

import java.io.IOException;
import java.nio.file.Files;
import java.nio.file.Path;
import java.time.LocalDateTime;
import java.time.format.DateTimeFormatter;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertInstanceOf;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ReportSaverImplTest {

    @Test
    @DisplayName("save() записывает файл с ожидаемым содержимым отчета")
    void saveWritesFileWithExpectedContent(@TempDir Path tempDir) throws IOException {
        Report report = new Report(LocalDateTime.of(2024, 3, 15, 9, 30, 0), 7, 2);
        ReportSaverImpl reportSaver = new ReportSaverImpl(tempDir);

        reportSaver.save(report);

        String expectedFileName = "report-" + report.dateTime().format(DateTimeFormatter.ofPattern("yyyy-MM-dd_HH-mm-ss")) + ".txt";
        Path expectedFile = tempDir.resolve(expectedFileName);
        assertTrue(Files.exists(expectedFile));
        assertEquals(report.toReportText(), Files.readString(expectedFile));
    }

    @Test
    @DisplayName("save() оборачивает ошибку записи файла в ApplicationException с кодом FILE_WRITE_ERROR")
    void saveWrapsFileWriteFailureIntoApplicationException(@TempDir Path tempDir) throws IOException {
        Path blockedDirectory = tempDir.resolve("blocked");
        Files.writeString(blockedDirectory, "это обычный файл, а не каталог");

        Report report = new Report(LocalDateTime.of(2024, 3, 15, 9, 30, 0), 7, 2);
        ReportSaverImpl reportSaver = new ReportSaverImpl(blockedDirectory);

        ApplicationException exception = assertThrows(ApplicationException.class, () -> reportSaver.save(report));

        assertEquals(ApplicationErrorCode.FILE_WRITE_ERROR, exception.getErrorCode());
        assertInstanceOf(IOException.class, exception.getCause());
    }
}
