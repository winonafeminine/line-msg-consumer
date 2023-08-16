'use client'
import { useRouter, useSearchParams } from "next/navigation";
import * as React from 'react';

var mockData = {
  "data": {
      "state": "64dc03709b7f18a352feb9ed",
      "app_redirect_uri": "https://2186-171-7-224-238.ngrok-free.app/auth/line/connect/demo",
      "access_token": "12345",
      "group_user_id": "Ube388d9e93bf06a6bde98435b1b286a3",
      "platform_id": "64dc03969b7f18a352feb9ee"
  },
  "status_code": 200
}

export default function Verify() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const handleConfirm = async() => {
    // handle confirmation logic here
    const state = searchParams.get('state');
    const code = searchParams.get('state');
    var data = mockData;
    try{
      var apiUrl = process.env.NEXT_PUBLIC_API_URL;
      var body = {
        code
      };
      var res = await fetch(`${apiUrl}/api/auth/v1/line/state/${state}`, {
        headers: {
          'Content-Type': 'application/json'
        },
        method: 'PUT',
        body: JSON.stringify(body)
      })
      data = await res.json();
      const redirectUrl = `${data['data']['app_redirect_uri']}?pid=${data['data']['platform_id']}&guid=${data['data']['group_user_id']}&token=${data['data']['access_token']}`;
      window.location.href = redirectUrl;

    }catch(ex){
      // console.log(ex)
      data = mockData;
      const url = `/platform/verify/demo?pid=${data['data']['platform_id']}&guid=${data['data']['group_user_id']}&token=${data['data']['access_token']}`;
      router.push(url);
    }
    // alert("Confirmed!");
  };

  const handleCancel = () => {
    // handle cancel logic here
    router.push("/");
  };

  React.useEffect(() => {
    console.log(process.env['NEXT_PUBLIC_API_URL']);
  }, [])

  return (
    <div className="flex flex-col items-center justify-center min-h-screen py-2">
      <p className="text-lg font-medium mb-4">Are you sure you want to proceed?</p>
      <div className="flex space-x-4">
        <button
          className="px-4 py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600"
          onClick={handleConfirm}
        >
          Yes
        </button>
        <button
          className="px-4 py-2 bg-gray-300 text-black rounded-md hover:bg-gray-400"
          onClick={handleCancel}
        >
          Cancel
        </button>
      </div>
    </div>
  );
}
