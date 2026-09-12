package studying.withsolid.service;

import org.junit.jupiter.api.DisplayName;
import org.junit.jupiter.api.Test;
import studying.withsolid.exception.ApplicationErrorCode;
import studying.withsolid.exception.ApplicationException;
import studying.withsolid.model.Report;

import java.time.LocalDateTime;
import java.util.ArrayList;
import java.util.List;

import static org.junit.jupiter.api.Assertions.assertEquals;
import static org.junit.jupiter.api.Assertions.assertThrows;

class ReportServiceFacadeTest {

    private final Report report = new Report(LocalDateTime.of(2024, 1, 1, 12, 0, 0), 10, 5);

    @Test
    @DisplayName("process() сохраняет отчет перед его отправкой")
    void processSavesReportBeforeSendingIt() {
        List<String> callOrder = new ArrayList<>();
        ReportSaver reportSaver = savedReport -> callOrder.add("save");
        ReportSender reportSender = (sentReport, email) -> callOrder.add("send");

        ReportServiceFacade facade = new ReportServiceFacade(reportSaver, reportSender);
        facade.process(report, "boss@company.com");

        assertEquals(List.of("save", "send"), callOrder);
    }

    @Test
    @DisplayName("process() отправляет отчет на указанный email получателя")
    void processSendsReportToCorrectRecipient() {
        String[] capturedEmail = new String[1];
        ReportSaver reportSaver = savedReport -> { };
        ReportSender reportSender = (sentReport, email) -> capturedEmail[0] = email;

        ReportServiceFacade facade = new ReportServiceFacade(reportSaver, reportSender);
        facade.process(report, "customer@company.com");

        assertEquals("customer@company.com", capturedEmail[0]);
    }

    @Test
    @DisplayName("process() выбрасывает ApplicationException с кодом VALIDATION_ERROR при отсутствии отчета")
    void processThrowsValidationErrorWhenReportIsMissing() {
        ReportSaver reportSaver = savedReport -> { };
        ReportSender reportSender = (sentReport, email) -> { };
        ReportServiceFacade facade = new ReportServiceFacade(reportSaver, reportSender);

        ApplicationException exception = assertThrows(ApplicationException.class,
                () -> facade.process(null, "boss@company.com"));

        assertEquals(ApplicationErrorCode.VALIDATION_ERROR, exception.getErrorCode());
    }

    @Test
    @DisplayName("process() выбрасывает ApplicationException с кодом VALIDATION_ERROR при отсутствии email")
    void processThrowsValidationErrorWhenEmailIsMissing() {
        ReportSaver reportSaver = savedReport -> { };
        ReportSender reportSender = (sentReport, email) -> { };
        ReportServiceFacade facade = new ReportServiceFacade(reportSaver, reportSender);

        ApplicationException exception = assertThrows(ApplicationException.class,
                () -> facade.process(report, " "));

        assertEquals(ApplicationErrorCode.VALIDATION_ERROR, exception.getErrorCode());
    }
}
