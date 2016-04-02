using System.Collections;
using System.Reflection;
using System.Text;

namespace MMogri.Utils
{
    public class SQLiteWrapper
    {
        public enum SQLiteType
        {
            INTEGER, TEXT, REAL
        }

        public struct SqlItem
        {
            public string name;
            public SQLiteType type;
            public MemberInfo inf;
            public bool isKey;
            public SQLiteDataConverter dataConverter;

            public SqlItem( string name, SQLiteType type, MemberInfo inf, bool key = false, SQLiteDataConverter dataConverter = null )
            {
                this.name = name;
                this.type = type;
                this.inf = inf;
                this.isKey = key;
                this.dataConverter = dataConverter;
            }

            public void SetValue( object target, object value )
            {
                if( dataConverter != null )
                    value = dataConverter.ConvertRead( value );
                switch( inf.MemberType )
                {
                    case MemberTypes.Field:
                        ( (FieldInfo)inf ).SetValue( target, value ); break;
                    case MemberTypes.Property:
                        ( (PropertyInfo)inf ).SetValue( target, value, null ); break;
                    default:
                        throw new System.ArgumentException( "Input MemberInfo must be if type FieldInfo or PropertyInfo" );
                }
            }

            public object GetValue( object target )
            {
                object value = null;
                switch( inf.MemberType )
                {
                    case MemberTypes.Field:
                        value = ( (FieldInfo)inf ).GetValue( target ); break;
                    case MemberTypes.Property:
                        value = ( (PropertyInfo)inf ).GetValue( target, null ); break;
                    default:
                        throw new System.ArgumentException( "Input MemberInfo must be if type FieldInfo or PropertyInfo" );
                }
                if( dataConverter != null )
                    value = dataConverter.CovertWrite( value );
                return value;
            }
        }

        public enum InsertType
        {
            IGNORE, REPLACE
        }

        public string tableName;
        public SqlItem[] _items;

        public SQLiteWrapper( string name, SqlItem[] items )
        {
            tableName = name;
            _items = items;
        }

        public IEnumerable IterateItems()
        {
            foreach( SqlItem i in _items )
                yield return i;
        }

        public string CreateCmd()
        {
            StringBuilder str = new StringBuilder();
            str.Append( "CREATE TABLE `" + tableName + "` (" );

            for( int i = 0; i < _items.Length; i++ )
            {
                str.Append( "`" + _items[i].name + "`" + " " + _items[i].type.ToString() );
                if( _items[i].isKey )
                    str.Append( " PRIMARY KEY" );
                if( i < _items.Length - 1 )
                    str.Append( "," );
            }
            str.Append( ")" );
            return str.ToString();
        }

        public string InsertCmd( InsertType t = InsertType.REPLACE )
        {
            StringBuilder str = new StringBuilder();
            str.Append( "INSERT OR " + t.ToString() + " INTO " + tableName + " ( " );
            for( int i = 0; i < _items.Length; i++ )
            {
                str.Append( _items[i].name );
                if( i < _items.Length - 1 ) str.Append( ", " );
            }
            str.Append( ") " );
            str.Append( "VALUES ( " );
            for( int i = 0; i < _items.Length; i++ )
            {
                str.Append( "@param" + i );
                if( i < _items.Length - 1 ) str.Append( ", " );
            }
            str.Append( ")" );

            return str.ToString();
        }

        public string SelectCmd( string spec = "" )
        {
            StringBuilder str = new StringBuilder();
            str.Append( "SELECT " );
            for( int i = 0; i < _items.Length; i++ )
            {
                str.Append( _items[i].name );
                if( i < _items.Length - 1 ) str.Append( ", " );
            }
            str.Append( " FROM " + tableName );
            str.Append( " " + spec );

            return str.ToString();
        }

        public string DeleteCmd( string spec = "" )
        {
            StringBuilder str = new StringBuilder();
            str.Append( "DELETE FROM " );
            str.Append( tableName );
            str.Append(spec );
            str.Append( "'" );

            return str.ToString();
        }

        public string DeleteTableCmd()
        {
            StringBuilder str = new StringBuilder();
            str.Append( "DELETE FROM " );
            str.Append( tableName );

            return str.ToString();
        }

        public string UpdateCmd( string spec )
        {
            StringBuilder str = new StringBuilder();
            str.Append( "UPDATE " );
            str.Append( tableName );
            str.Append( " SET " );
            for( int i = 0; i < _items.Length; i++ )
            {
                str.Append( _items[i].name );
                str.Append( " = " );
                str.Append( "@param" + i );
                if( i < _items.Length - 1 ) str.Append( ", " );
            }
            str.Append( " " );
            str.Append( spec );

            return str.ToString();
        }
    }
}
