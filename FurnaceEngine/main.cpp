#include "Editor/mmf.h"
#include <Windows.h>
#include <iostream>
#include <format>


int APIENTRY wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nCmdShow)
{
#ifdef FURNACE_EDITOR
    furnace::mmf::initialize();
#endif
    static int i = 0;
    while (true) 
    {
        flatbuffers::FlatBufferBuilder fbb(256);
        auto str = fbb.CreateString(std::format("Engine Log Message: {}", i++));
        auto log = Furnace::Buffers::CreateLog(fbb, Furnace::Buffers::LogType_MAX, str);
        Furnace::Buffers::MessageWrapperBuilder mbb(fbb);
        mbb.add_message(log.o),
        mbb.add_message_type(Furnace::Buffers::AnyMessage_Log);
        auto mw = mbb.Finish();
        fbb.Finish(mw);
        furnace::mmf::send(fbb);
        Sleep(1000);
        fbb.Clear();
    }
    return 0;
}