#include <windows.h>
#include <stdio.h>

static int build_command_line(wchar_t *out, size_t out_size, const wchar_t *dir)
{
    int written = _snwprintf_s(out, out_size, _TRUNCATE,
                               L"\"%s\\dotnet.exe\" \"%s\\FruitNinja.dll\"",
                               dir, dir);
    return (written > 0) ? 0 : -1;
}

int WINAPI wWinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance,
                    wchar_t *lpCmdLine, int nCmdShow)
{
    (void)hInstance;
    (void)hPrevInstance;
    (void)lpCmdLine;
    (void)nCmdShow;

    wchar_t exe_path[MAX_PATH];
    DWORD len = GetModuleFileNameW(NULL, exe_path, MAX_PATH);
    if (len == 0 || len >= MAX_PATH) {
        return 1;
    }

    wchar_t *slash = exe_path;
    for (wchar_t *p = exe_path; *p; ++p) {
        if (*p == L'\\' || *p == L'/') {
            slash = p + 1;
        }
    }
    *slash = L'\0';

    wchar_t cmd_line[2048];
    if (build_command_line(cmd_line, 2048, exe_path) != 0) {
        MessageBoxW(NULL, L"Path too long for the launcher.", L"Fruit Ninja", MB_ICONERROR);
        return 1;
    }

    STARTUPINFOW si;
    ZeroMemory(&si, sizeof(si));
    si.cb = sizeof(si);

    PROCESS_INFORMATION pi;
    ZeroMemory(&pi, sizeof(pi));

    if (!CreateProcessW(NULL, cmd_line, NULL, NULL, FALSE,
                        CREATE_NO_WINDOW, NULL, exe_path, &si, &pi)) {
        wchar_t msg[256];
        _snwprintf_s(msg, 256, _TRUNCATE,
                     L"Failed to start dotnet.exe FruitNinja.dll (error %lu).",
                     GetLastError());
        MessageBoxW(NULL, msg, L"Fruit Ninja", MB_ICONERROR);
        return 1;
    }

    WaitForSingleObject(pi.hProcess, INFINITE);

    DWORD exit_code = 0;
    GetExitCodeProcess(pi.hProcess, &exit_code);

    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);

    return (int)exit_code;
}
