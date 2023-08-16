'use client'
import {useSearchParams} from 'next/navigation';

export default function Demo()
{
    const searchParams = useSearchParams();
    return (
        <div>
            <code>
                <span>Group user id: {searchParams.get('guid')}</span><br/>
                <span>Platform id: {searchParams.get('pid')}</span><br/>
                <span>Access token: {searchParams.get('token')}</span>
            </code>
        </div>
    )
}