﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.FSharp.Core;

namespace ParserTester {

    static class FSharpOptionExt {
        //Makes an F# option behave like a nullable value where None == Null
        public static string getVal(this FSharpOption<string> fOp) {
            if(fOp == FSharpOption<string>.None) {
                return null;
            } else {
                return fOp.Value;
            }
        }
    }

    /*I wanted to do this :(
    static class FSharpOptionExt<T> {
        //Makes an F# option behave like a nullable value where None == Null
        public static T getVal(this FSharpOption<T> fOp) {
            if(fOp == FSharpOption<T>.None) {
                return default(T);
            } else {
                return fOp.Value;
            }
        }
    }
    */

    class ModelBuilder {
        //I introduced this class so that only this class has dependencies on the F# project
        private Dictionary<string, SqlSchema> schemas = new Dictionary<string,SqlSchema>();

        public SqlQuery build(string SQLString) {
            Sql.sqlStatement stmnt;
            try {
                stmnt = Parser.ParseSql(SQLString);
            } catch(Exception) {
                //TODO catch the rigth exception here
                //I dont want to throw the parse exception because it would introduce a dependencies between the model and the parser
                throw new Exception("Parse Error");
            }
            SqlQuery qry = new SqlQuery();
            qry.Tables.Add(new SqlTable() {
                    Schema = getSchema(stmnt.Table1.SchemaName),
                    AliasName = stmnt.Table1.AliasName.getVal(),
                    TableName = stmnt.Table1.TableName,
                    Columns = stmnt
                                .getTableFields(stmnt.Table1.Identifier)
                                .Select(fld => new SqlColumn() { Alias = fld.Item2, ColumnName = fld.Item1 })
                                .ToList(),
                    //JoinItems = stmnt.Joins.Where(jn => jn.
                }
            );

            

            qry.JoinItems.AddRange(null);
            //TODO Add rest
            return qry;
        }

            
        
        private SqlSchema getSchema(string schemaName) {
            if (String.IsNullOrEmpty(schemaName)){
                return null;   
            }
            if(schemas.ContainsKey(schemaName)) {
                return schemas[schemaName];
            } else {
                SqlSchema nw = new SqlSchema() { SchemaName = schemaName };
                schemas.Add(schemaName, nw);
                return nw;
            }
        }

    }
}