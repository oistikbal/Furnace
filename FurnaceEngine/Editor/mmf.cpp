#include "buffers_generated.h"

#include <iostream>
#include <windows.h>
#include <thread>
#include <queue>
#include <mutex>
#include <string>
#include <vector>
#include "..\Utilities\action.h"
#include "mmf.h"

constexpr static int k_ownIdentifier = 1;
constexpr static int k_fileSize = 1024 * 1024;
constexpr static int k_messageSize = 256;
constexpr static int k_headerSize = 2 * sizeof(uint8_t); // sender_id and package_id as bytes
constexpr static const wchar_t* k_mmfName = L"FurnaceEngine";

static std::queue<flatbuffers::FlatBufferBuilder> s_messageQueue;
static std::mutex s_queueMutex;
static bool s_isRunning = true;
static bool s_isInitialized;
static HANDLE s_mapFile;

furnace::utl::action<const Furnace::Buffers::MessageWrapper&> furnace::mmf::on_message_received;

static void write() {
    LPVOID pBuf = MapViewOfFile(s_mapFile, FILE_MAP_ALL_ACCESS, 0, 0, k_fileSize);
    if (pBuf == NULL) {
        std::cerr << "Could not map view of file: " << GetLastError() << std::endl;
        CloseHandle(s_mapFile);
        return;
    }

    uint8_t sender_id = 1;
    uint8_t package_id = 0;

    while (s_isRunning) {
        flatbuffers::FlatBufferBuilder builder;

        {
            std::lock_guard<std::mutex> lock(s_queueMutex);
            if (!s_messageQueue.empty()) {
                builder = std::move(s_messageQueue.front());
                s_messageQueue.pop();
            }
        }

        if (builder.GetSize() > 0) {
            int messageLength = builder.GetSize();

            if (messageLength > 0 && messageLength <= k_messageSize) {
                std::memcpy((char*)pBuf, &sender_id, sizeof(sender_id));
                std::memcpy((char*)pBuf + sizeof(sender_id), &package_id, sizeof(package_id));
                std::memcpy((char*)pBuf + k_headerSize, builder.GetBufferPointer(), messageLength);
                package_id++;
            }
            else // Package is missed!
            {
                assert(0);
            }
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(100));
    }


    UnmapViewOfFile(pBuf);
    CloseHandle(s_mapFile);
}

static void read() {
    LPVOID pBuf = MapViewOfFile(s_mapFile, FILE_MAP_ALL_ACCESS, 0, 0, k_fileSize);
    if (pBuf == NULL) {
        std::cerr << "Could not map view of file: " << GetLastError() << std::endl;
        CloseHandle(s_mapFile);
        return;
    }

    uint8_t lastPackageId = 255;

    while (s_isRunning) {
        uint8_t senderId = *((uint8_t*)pBuf);
        uint8_t packageId = *((uint8_t*)pBuf + 1);

        if (senderId != k_ownIdentifier && packageId != lastPackageId) {
            auto messageWrapper = Furnace::Buffers::GetMessageWrapper((uint8_t*)pBuf + k_headerSize);
            if (messageWrapper == nullptr) {
                std::cerr << "Error: messageWrapper is null!" << std::endl;
                continue; // Skip to the next iteration
            }

            auto messageType = messageWrapper->message_type();
            furnace::mmf::on_message_received.invoke(*messageWrapper);
            lastPackageId = packageId;
        }

        std::this_thread::sleep_for(std::chrono::milliseconds(10));

    }
    UnmapViewOfFile(pBuf);
    CloseHandle(s_mapFile);
    std::cout << "Exiting ReadFromSharedMemory function." << std::endl;
}


// Function to add messages to the queue
void furnace::mmf::send(flatbuffers::FlatBufferBuilder& builder) 
{
    std::lock_guard<std::mutex> lock(s_queueMutex);
    s_messageQueue.push(std::move(builder));
}

void furnace::mmf::initialize() {
    if (s_isInitialized) 
    {
        assert(0);
        return;
    }

    s_mapFile = OpenFileMapping(FILE_MAP_ALL_ACCESS, FALSE, k_mmfName);
    if (s_mapFile == NULL) {
        s_mapFile = CreateFileMapping(
            INVALID_HANDLE_VALUE,
            NULL,
            PAGE_READWRITE,
            0,
            k_fileSize,
            k_mmfName
        );

        if (s_mapFile == NULL) {
            std::cerr << "Could not create or open file mapping object: " << GetLastError() << std::endl;
            return;
        }
    }

    std::thread writerThread(write);
    std::thread readerThread(read);

    s_isRunning = true;
    writerThread.detach();
    readerThread.detach();

    s_isInitialized = true;
}