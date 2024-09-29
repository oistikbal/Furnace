#include "Editor/mmf.h"
#include <Windows.h>
#include <iostream>


int APIENTRY wWinMain(_In_ HINSTANCE hInstance, _In_opt_ HINSTANCE hPrevInstance, _In_ LPWSTR lpCmdLine, _In_ int nCmdShow)
{
#ifdef FURNACE_EDITOR
    furnace::mmf::initialize();
#endif

    while (true) 
    {
        flatbuffers::FlatBufferBuilder fbb(256);
        auto str = fbb.CreateString("Engine Log Message");
        auto log = Furnace::Buffers::CreateLog(fbb, Furnace::Buffers::LogType_Info, str);
        Furnace::Buffers::MessageWrapperBuilder mbb(fbb);
        mbb.add_message(log.o),
        mbb.add_message_type(Furnace::Buffers::AnyMessage_Log);
        auto mw = mbb.Finish();
        fbb.Finish(mw);
        furnace::mmf::send(fbb);
        Sleep(10);
        fbb.Clear();
    }
    return 0;
}