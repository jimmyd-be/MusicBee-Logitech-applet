//************************************************************************
//  The Logitech LCD SDK, including all acompanying documentation,
//  is protected by intellectual property laws.  All use of the Logitech
//  LCD SDK is subject to the License Agreement found in the
//  "Logitech LCD SDK License Agreement" file and in the Reference Manual.  
//  All rights not expressly granted by Logitech are reserved.
//************************************************************************

//************************************************************************
//
// Bitmap.cpp
//
//************************************************************************

#include "StdAfx.h"
#include "Bitmap.h"

//************************************************************************
//
// cBitmap::cBitmap
//
//************************************************************************

cBitmap::cBitmap(void)
{
    m_hBM = NULL;
    m_nWidth = 0;
    m_nHeight = 0;
    m_pGdiPlusBitmap = NULL;
}


//************************************************************************
//
// cBitmap::~cBitmap
//
//************************************************************************

cBitmap::~cBitmap(void)
{
    Shutdown();
}


//************************************************************************
//
// cBitmap::Shutdown
//
//************************************************************************

void cBitmap::Shutdown(void)
{
    m_nWidth = 0;
    m_nHeight = 0;
    if(NULL != m_pGdiPlusBitmap)
    {
        delete m_pGdiPlusBitmap;
        m_pGdiPlusBitmap = NULL;
    }
    SAFE_DELETE_GDIOBJECT(m_hBM);
}


//************************************************************************
//
// cBitmap::Detach
//
//************************************************************************

void cBitmap::Detach(void)
{
    Shutdown();
}


//************************************************************************
//
// cBitmap::LoadFromResource
//
//************************************************************************

HRESULT cBitmap::LoadFromResource(HDC hDC, 
                                  HINSTANCE hInstance, 
                                  UINT nResourceID, 
                                  LPCTSTR sResourceType /* = MAKEINTRESOURCE(RT_BITMAP */)
{
    UNREFERENCED_PARAMETER(hDC);

    // Clear the old bitmap
    Shutdown();

    HRSRC hResource = FindResource(hInstance, MAKEINTRESOURCE(nResourceID), sResourceType);
    if(NULL == hResource)
    {
        TRACE(_T("CBitmap::LoadImageFromResource(): failed to locate resource 0x%x (type: 0x%x) in instance 0x%x.\n"),
            nResourceID, sResourceType, hInstance);
        return E_FAIL;
    }

    DWORD dwImageSize = SizeofResource(hInstance, hResource);
    if (0 == dwImageSize)
    {
        return E_FAIL;
    }

    const void* pResourceData = LockResource(LoadResource(hInstance, hResource));
    if (NULL == pResourceData)
    {
        return E_FAIL;
    }

    HGLOBAL hGlobal = GlobalAlloc(GMEM_MOVEABLE | GMEM_DISCARDABLE, dwImageSize);
    if (NULL == hGlobal)
    {
        return E_FAIL;
    }

    void* pBuffer = GlobalLock(hGlobal);
    if (NULL == pBuffer)
    {
        GlobalFree(hGlobal);
        return E_FAIL;
    }

    CopyMemory(pBuffer, pResourceData, dwImageSize);

    // CreateStreamOnHGlobal requires HGLOBAL to be GlobalAlloc, moveable, and discardable 
    if(NULL == hGlobal)
    {
        TRACE(_T("cBitmap::LoadImageFromResource(): failed to load resource 0x%x (type: 0x%x) in instance 0x%x.\n"),
            nResourceID, sResourceType, hInstance);
        GlobalFree(hGlobal);
        return E_FAIL;
    }

    // Create an IStram from the HGLOBAL
    IStream* pStream = NULL;
    DWORD dwRet = S_OK;
    dwRet = ::CreateStreamOnHGlobal(hGlobal, FALSE, &pStream);
    if (FAILED(dwRet))
    {
        TRACE(_T("ERROR: CBitmap::LoadImageFromResource CreateStreamOnHGlobal returned %d\n"), dwRet);
        GlobalFree(hGlobal);
        return dwRet;
    }

    // Use GDI+ to generate a bitmap
    m_pGdiPlusBitmap = Gdiplus::Bitmap::FromStream(pStream);
    pStream->Release();
    if (!m_pGdiPlusBitmap)
    {
        TRACE(_T("ERROR: CBitmap::LoadImageFromResource Gdiplus::Bitmap::FromStream failed\n"));
        GlobalFree(hGlobal);
        return E_FAIL;
    }

    // We don;t need the global memory block anymore
    GlobalFree(hGlobal);
    hGlobal = NULL;

    // Get the bitmap handle
    HBITMAP hBitmap = NULL;
    if (m_pGdiPlusBitmap->GetHBITMAP(RGB(0,0,0), &hBitmap) != Gdiplus::Ok)
    {
        TRACE(_T("ERROR: CBitmap::LoadImageFromResource Gdiplus::Bitmap::GetHBITMAP failed\n"));
        if (m_pGdiPlusBitmap)
        {
            delete m_pGdiPlusBitmap;
            m_pGdiPlusBitmap = NULL;
        }
        return E_FAIL;
    }

    // Get the bitmap dimensions
    BITMAP bmpInfo;
    ZeroMemory(&bmpInfo, sizeof(bmpInfo));
    ::GetObject(hBitmap, sizeof(BITMAP), &bmpInfo);

    m_nWidth = bmpInfo.bmWidth;
    m_nHeight = bmpInfo.bmHeight;

    m_hBM = hBitmap;

    return S_OK;
}


//************************************************************************
//
// cBitmap::LoadFromFile
//
//************************************************************************

HRESULT cBitmap::LoadFromFile(HDC hDC, LPCTSTR sFilename)
{
    UNREFERENCED_PARAMETER(hDC);

    // Clear the old bitmap
    Shutdown();

    // Use GDI+ to generate a bitmap
    CStringW wsFilename(sFilename);
    m_pGdiPlusBitmap = Gdiplus::Bitmap::FromFile(wsFilename);
    if(NULL == m_pGdiPlusBitmap)
    {
        return E_FAIL;
    }

    // Get the bitmap handle
    HBITMAP hBitmap = NULL;
    if (m_pGdiPlusBitmap->GetHBITMAP(RGB(0,0,0), &hBitmap) != Gdiplus::Ok)
    {
        TRACE(_T("ERROR: cBitmap::LoadImageFromFile Gdiplus::Bitmap::GetHBITMAP failed\n"));
        delete m_pGdiPlusBitmap;
        m_pGdiPlusBitmap = NULL;
        return E_FAIL;
    }

    // Get the bitmap dimensions
    BITMAP bmpInfo;
    ZeroMemory(&bmpInfo, sizeof(bmpInfo));
    ::GetObject(hBitmap, sizeof(BITMAP), &bmpInfo);

    m_nWidth = bmpInfo.bmWidth;
    m_nHeight = bmpInfo.bmHeight;

    m_hBM = hBitmap;

    return S_OK;
}


//************************************************************************
//
// cBitmap::LoadFromMemory
//
//************************************************************************

HRESULT cBitmap::LoadFromMemory(HDC hDC, LPVOID pBuffer, size_t nBufferSize)
{
    UNREFERENCED_PARAMETER(hDC);

    // Clear the old bitmap
    Shutdown();
    HGLOBAL hGlobal = GlobalAlloc(GMEM_MOVEABLE/* | GMEM_DISCARDABLE*/, nBufferSize);
    if (NULL == hGlobal)
    {
        return E_FAIL;
    }

    void* pGBuffer = GlobalLock(hGlobal);
    if (NULL == pGBuffer)
    {
        GlobalFree(hGlobal);
        return E_FAIL;
    }

    CopyMemory(pGBuffer, pBuffer, nBufferSize);

    GlobalUnlock(hGlobal);

    // Create an IStream from the HGLOBAL
    IStream* pStream = NULL;
    DWORD dwRet = S_OK;
    dwRet = ::CreateStreamOnHGlobal(hGlobal, FALSE, &pStream);
    if (FAILED(dwRet))
    {
        TRACE(_T("ERROR: cBitmap::LoadFromMemory CreateStreamOnHGlobal returned %d\n"), dwRet);
        GlobalFree(hGlobal);
        hGlobal = NULL;
        return dwRet;
    }

    // Use GDI+ to generate a bitmap
    m_pGdiPlusBitmap = Gdiplus::Bitmap::FromStream(pStream);
    if (!m_pGdiPlusBitmap)
    {
        TRACE(_T("ERROR: cBitmap::LoadFromMemory Gdiplus::Bitmap::FromStream failed\n"));

        pStream->Release();
        GlobalFree(hGlobal);
        hGlobal = NULL;

        return E_FAIL;
    }

    // Get the bitmap handle
    HBITMAP hBitmap = NULL;
    Gdiplus::Status status = m_pGdiPlusBitmap->GetHBITMAP(RGB(0,0,0), &hBitmap);
    if (Gdiplus::Ok != status)
    {
        TRACE(_T("ERROR: cBitmap::LoadFromMemory Gdiplus::Bitmap::GetHBITMAP failed\n"));
        delete m_pGdiPlusBitmap;
        m_pGdiPlusBitmap = NULL;

        pStream->Release();
        GlobalFree(hGlobal);
        hGlobal = NULL;

        return E_FAIL;
    }

    // Get the bitmap dimensions
    BITMAP bmpInfo;
    ZeroMemory(&bmpInfo, sizeof(bmpInfo));
    ::GetObject(hBitmap, sizeof(BITMAP), &bmpInfo);

    m_nWidth = bmpInfo.bmWidth;
    m_nHeight = bmpInfo.bmHeight;

    m_hBM = hBitmap;

    // We don't need the global memory block anymore
    pStream->Release();
    GlobalFree(hGlobal);
    hGlobal = NULL;

    return S_OK;
}


//************************************************************************
//
// cBitmap::GetWidth
//
//************************************************************************

int cBitmap::GetWidth(void)
{
    return m_nWidth;
}


//************************************************************************
//
// cBitmap::GetHeight
//
//************************************************************************

int cBitmap::GetHeight(void)
{
    return m_nHeight;
}


//************************************************************************
//
// cBitmap::GetHBITMAP
//
//************************************************************************

HBITMAP cBitmap::GetHBITMAP(void)
{
    return m_hBM;
}


//** end of Bitmap.cpp ***************************************************

