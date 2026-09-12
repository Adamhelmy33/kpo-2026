package studying.withsolid.service.impl;

import org.junit.jupiter.api.AfterEach;
import org.junit.jupiter.api.BeforeEach;
import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import studying.withsolid.exception.ApplicationErrorCode;
import studying.withsolid.exception.ApplicationException;
import studying.withsolid.model.Report;

import java.io.ByteArrayOutputStream;
import java.io.PrintStream;
import java.nio.charset.StandardCharsets;
import java.time.LocalDateTime;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;
import static org.junit.jupiter.api.Assertions.assertTrue;

class ReportSenderImplTest {

    private final ReportSenderImpl reportSender = new ReportSenderImpl();
    private final PrintStream originalOut = System.out;
    private ByteArrayOutputStream capturedOut;

    @BeforeEach
    void redirectSystemOut() {
        capturedOut = new ByteArrayOutputStream();
        System.setOut(new PrintStream(capturedOut, true, StandardCharsets.UTF_8));
    }

    @AfterEach
    void restoreSystemOut() {
        System.setOut(originalOut);
    }

    @Test
    @DisplayName("send() выводит в консоль текст отчета и email получателя")
    void sendPrintsReportTextAndRecipientEmail() {
        Report report = new Report(LocalDateTime.of(2024, 5, 1, 10, 0, 0), 3, 1);

        reportSender.send(report, "customer@company.com");

        String output = capturedOut.toString(StandardCharsets.UTF_8);
        assertTrue(output.contains("customer@company.com"));
        assertTrue(output.contains(report.toReportText()));
    }

    @Test
    @DisplayName("send() выбрасывает ApplicationException с кодом VALIDATION_ERROR при отсутствии отчета")
    void sendThrowsValidationErrorWhenReportIsMissing() {
        ApplicationException exception = assertThrows(ApplicationException.class,
                () -> reportSender.send(null, "customer@company.com"));

        assertEquals(ApplicationErrorCode.VALIDATION_ERROR, exception.getErrorCode());
    }

    @Test
    @DisplayName("send() выбрасывает ApplicationException с кодом VALIDATION_ERROR при отсутствии email")
    void sendThrowsValidationErrorWhenEmailIsMissing() {
        Report report = new Report(LocalDateTime.of(2024, 5, 1, 10, 0, 0), 3, 1);

        ApplicationException exception = assertThrows(ApplicationException.class,
                () -> reportSender.send(report, ""));

        assertEquals(ApplicationErrorCode.VALIDATION_ERROR, exception.getErrorCode());
    }
}
